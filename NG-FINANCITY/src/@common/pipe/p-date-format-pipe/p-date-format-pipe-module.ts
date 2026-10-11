import { NgModule } from '@angular/core';
import { PDateFormatPipe } from './p-date-format-pipe';
import { IntlModule } from '@progress/kendo-angular-intl';

@NgModule({
    declarations: [PDateFormatPipe],
    imports: [
        IntlModule
    ],
    exports: [PDateFormatPipe]
})
export class PDateFormatPipeModule {
}



