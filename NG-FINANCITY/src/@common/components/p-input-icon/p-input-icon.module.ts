import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PInputIconDirective } from './p-input-icon.directive';

@NgModule({
    declarations: [PInputIconDirective],
    imports: [
        CommonModule,
    ],
    exports: [PInputIconDirective]
})
export class PInputIconDirectiveModule { }
