import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogModule } from '@progress/kendo-angular-dialog';
import { RecapDailogComponent } from './p-recap-dialog.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { RecapDailogGridComponent } from './p-recap-dialog-grid.component';
import { PGridViewModule } from '../p-grid-view/p-grid-view.module';

@NgModule({
    entryComponents: [
        RecapDailogComponent,
        RecapDailogGridComponent
    ],
    declarations: [
        RecapDailogComponent,
        RecapDailogGridComponent,
    ],
    imports: [
        CommonModule,
        DialogModule,
        PGridViewModule,
        GridModule
    ],
    exports: [
        RecapDailogComponent,
        RecapDailogGridComponent
    ]
})
export class RecapDailogModule { }
