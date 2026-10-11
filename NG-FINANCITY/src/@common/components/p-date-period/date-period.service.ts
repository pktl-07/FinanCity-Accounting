import { Injectable } from '@angular/core';
import { zeroPrepending } from 'src/@common/service/utility-service';

@Injectable()
export class DatePeriodService {

    startYear = 2020;

    years = () => {
        let y = [];
        for (let i = this.startYear; i <= new Date().getFullYear(); i++) {
            y.push({ value: i, text: i })
        }
        return y;
    };

    currentYear = new Date().getFullYear();

    months = [
        { value: 1, text: 'Jan' },
        { value: 2, text: 'Feb' },
        { value: 3, text: 'Mar' },
        { value: 4, text: 'Apr' },
        { value: 5, text: 'May' },
        { value: 6, text: 'Jun' },
        { value: 7, text: 'Jul' },
        { value: 8, text: 'Aug' },
        { value: 9, text: 'Sep' },
        { value: 10, text: 'Oct' },
        { value: 11, text: 'Nov' },
        { value: 12, text: 'Dec' }
    ];

    quarters = [
        { value: 3, text: 'First Quarter' },
        { value: 6, text: 'Second Quarter' },
        { value: 9, text: 'Third Quarter' },
        { value: 12, text: 'Forth Quarter' },
    ];

    halfs = [
        { value: 6, text: 'First Period' },
        { value: 12, text: 'Second Peroid' }
    ];

    transalateMonthly(month: number, year: number) {

        const monthPeriod = zeroPrepending(month, 2);

        return `${year}${monthPeriod}01-${year}${monthPeriod}31`;
    }

    transalateQuarterly(period: number, year: number) {

        const startMonth = zeroPrepending(period - 2, 2);
        const endMonth = zeroPrepending(period, 2);

        return `${year}${startMonth}01-${year}${endMonth}31`;
    }

    transalteHalfYear(period: number, year: number) {

        const startMonth = zeroPrepending(period - 5, 2);
        const endMonth = zeroPrepending(period, 2);

        return `${year}${startMonth}01-${year}${endMonth}31`
    }

    transalateYearly(year: number) {

        return `${year}0101-${year}1231`;
    }

    transalateDateRange(date1: any, date2: any) {

        return `${date1}-${date2}`;
    }
}