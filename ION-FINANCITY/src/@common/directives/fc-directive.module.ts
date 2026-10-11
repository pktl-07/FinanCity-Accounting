import { NgModule } from '@angular/core';
import { FCCurrencyFormatDirective } from './fc-currency-format.directive';
import { FCDateFormatDirective } from './fc-date-format.directive';
import { FCDecimalFormatDirective } from './fc-decimal-format.directive';
import { FCTimeFormatDirective } from './fc-time-format.directive';

@NgModule({
    declarations: [FCCurrencyFormatDirective],
    exports: [FCCurrencyFormatDirective],
})
export class FCCurrencyFormatModule { }

@NgModule({
    declarations: [FCDateFormatDirective],
    exports: [FCDateFormatDirective],
})
export class FCDateFormatModule { }

@NgModule({
    declarations: [FCTimeFormatDirective],
    exports: [FCTimeFormatDirective],
})
export class FCTimeFormatModule { }

@NgModule({
    declarations: [FCDecimalFormatDirective],
    exports: [FCDecimalFormatDirective],
})
export class FCDecimalFormatModule { }


