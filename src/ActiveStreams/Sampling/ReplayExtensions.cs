// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;

namespace ActiveStreams.Sampling
{
	public static class ReplayExtensions
	{
		public static void Replay<T>(this IEnumerable<T> stream, Func<T, DateTimeOffset> getTimestamp,
			Action<T> onNext,
			Action<Exception> onError,
			Action onComplete,
			DateTimeOffset? startAt = null,
			DateTimeOffset? endAt = null,
			double timeScale = 1.0d,
			TimeSpan? dutyCycle = null)
		{
			var notifications = ToNotificationStream(stream, getTimestamp, startAt, endAt);

			var scheduler = new TestScheduler();
			var sequence = scheduler.CreateHotObservable(notifications.ToArray());
			using (sequence.Subscribe(onNext, onError, onComplete))
			{
				var period = dutyCycle ?? TimeSpan.FromMilliseconds(10);
				using (Observable.Interval(period)
					.Subscribe(_ => scheduler.AdvanceBy((long) (period.Ticks * timeScale))))
				{
				}
			}
		}

		public static IEnumerable<T> Replay<T>(this IEnumerable<T> stream, Func<T, DateTimeOffset> getTimestamp,
			DateTimeOffset? startAt = null,
			DateTimeOffset? endAt = null,
			double timeScale = 1.0d,
			TimeSpan? dutyCycle = null)
		{
			var notifications = ToNotificationStream(stream, getTimestamp, startAt, endAt);

			var scheduler = new TestScheduler();
			var sequence = scheduler.CreateColdObservable(notifications.ToArray());

			var period = dutyCycle ?? TimeSpan.FromMilliseconds(10);
			Observable.Interval(period).Subscribe(_ => scheduler.AdvanceBy((long) (period.Ticks * timeScale)));
			return sequence.ToEnumerable();
		}

		private static IEnumerable<Recorded<Notification<T>>> ToNotificationStream<T>(IEnumerable<T> stream,
			Func<T, DateTimeOffset> getTimestamp, DateTimeOffset? startAt,
			DateTimeOffset? endAt)
		{
			// ReSharper disable once PossibleMultipleEnumeration
			startAt = startAt ?? getTimestamp(stream.First());

			// ReSharper disable once PossibleMultipleEnumeration
			endAt = endAt ?? getTimestamp(stream.Last());

			// ReSharper disable once PossibleMultipleEnumeration
			var notifications = stream.Select(x => ReactiveTest.OnNext(getTimestamp(x).Ticks - startAt.Value.Ticks, x))
				.Concat(new[] {ReactiveTest.OnCompleted<T>(endAt.Value.Ticks - startAt.Value.Ticks)})
				.ToArray();

			return notifications;
		}
	}
}