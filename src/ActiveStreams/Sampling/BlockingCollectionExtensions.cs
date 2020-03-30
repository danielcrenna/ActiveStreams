// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveStreams.Sampling
{
	internal static class BlockingCollectionExtensions
	{
		public static IObservable<T> AsConsumingObservable<T>(this BlockingCollection<T> sequence,
			CancellationToken cancellationToken)
		{
			var subject = new Subject<T>();
			var token = new CancellationToken();
			var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, token);
			var consumingTask = new Task(() =>
			{
				while (!sequence.IsCompleted)
					try
					{
						var item = sequence.Take(cancellationToken);
						try
						{
							subject.OnNext(item);
						}
						catch (Exception ex)
						{
							subject.OnError(ex);
						}
					}
					catch (OperationCanceledException)
					{
						break;
					}

				subject.OnCompleted();
			}, TaskCreationOptions.LongRunning);

			return new TaskAwareObservable<T>(subject, consumingTask, tokenSource);
		}

		private class TaskAwareObservable<T> : IObservable<T>, IDisposable
		{
			private readonly Subject<T> _subject;
			private readonly Task _task;
			private readonly CancellationTokenSource _taskCancellationTokenSource;

			public TaskAwareObservable(Subject<T> subject, Task task, CancellationTokenSource tokenSource)
			{
				_task = task;
				_subject = subject;
				_taskCancellationTokenSource = tokenSource;
			}

			public void Dispose()
			{
				_taskCancellationTokenSource.Cancel();
				_task.Wait();

				_taskCancellationTokenSource.Dispose();

				_subject.Dispose();
			}

			public IDisposable Subscribe(IObserver<T> observer)
			{
				var disposable = _subject.Subscribe(observer);
				if (_task.Status == TaskStatus.Created) _task.Start();
				return disposable;
			}
		}
	}
}