export interface ISchedulerRule {
    TYPE: string;
    Contains(date: Date): boolean;
}

export const DateTimeSchedulerType = 'WeSmart.Alpr.Core.Scheduler.DateTimeScheduler';

export interface IScheduler {
    TYPE: string;
    Rules: Array<ISchedulerRule>;
    IsAllowed(date: Date): boolean;
}

export class Scheduler implements IScheduler {
    Rules: Array<ISchedulerRule>;
    TYPE: string;

    constructor(rules?: Array<ISchedulerRule>) {
        this.TYPE = DateTimeSchedulerType;
        this.Rules = rules || [];
    }

    IsAllowed(date: Date): boolean {
        return false;
    }
}

export const WeekDaysHoursSchedulerRuleType = 'WeSmart.Alpr.Core.Scheduler.Rules.WeekDaysHoursRule';

export class WeekDaysHoursSchedulerRule implements ISchedulerRule {
    TYPE: string;
    Days: number;
    Hours: Array<number> = [];

    constructor(value?: any) {
        this.TYPE = WeekDaysHoursSchedulerRuleType;

        if (value) {
            this.Days = value.Days;
            this.Hours = value.Hours || [];
        }
    }

    Contains(date: Date): boolean {
        return false;
    }
}

export const DefaultScheduler = new Scheduler([
        new WeekDaysHoursSchedulerRule({
            Days: 127,
            Hours: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23]
        })
    ]
)
