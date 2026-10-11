import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { FCFilterToolbarComponent } from './fc-filter-toolbar.component';
import { FCSearchableToolbarComponent } from './fc-searchable-toolbar.component';
import { FCFilterFooterComponent } from './fc-filter-footer.component';

@NgModule({
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    declarations: [
        FCFilterToolbarComponent,
        FCSearchableToolbarComponent,
        FCFilterFooterComponent
    ],
    imports: [
        CommonModule,
        IonicModule,
    ],
    exports: [
        FCFilterToolbarComponent,
        FCSearchableToolbarComponent,
        FCFilterFooterComponent
    ],
})
export class FCFilterToolbarModule { }
