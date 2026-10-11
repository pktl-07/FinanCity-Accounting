import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { FCRecordStatusComponent } from './fc-record-status.component';
import { FCPayableStatusComponent } from './fc-payable-status.component';

@NgModule({
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    declarations: [
        FCRecordStatusComponent,
        FCPayableStatusComponent
    ],
    imports: [
        CommonModule,
        IonicModule,
    ],
    exports: [
        FCRecordStatusComponent,
        FCPayableStatusComponent
    ],
})
export class FCRecordStatusModule { }
