import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { FCFilterItemDirective } from './fc-filter-item.directive';
import { FCFilterCompositeDirective } from './fc-filter-composite.directive';
import { FCFilterModalComponent } from './fc-filter-modal.component';
import { FCCurrencyFormatModule, FCDateFormatModule } from 'src/@common/directives/fc-directive.module';


@NgModule({
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    entryComponents: [FCFilterModalComponent],
    declarations: [
        FCFilterCompositeDirective,
        FCFilterItemDirective,
        FCFilterModalComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        IonicModule,
        FCCurrencyFormatModule,
        FCDateFormatModule
    ],
    exports: [
        FCFilterCompositeDirective,
        FCFilterItemDirective,
        FCFilterModalComponent,
    ],
})
export class FCFilterModule { }
