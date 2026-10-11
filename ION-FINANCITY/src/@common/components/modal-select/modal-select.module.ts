import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { ModalSelectComponent } from './modal-select.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FCModalSelectContentDirective } from './modal-select-content-template';

const exports = [ModalSelectComponent,
    FCModalSelectContentDirective]

@NgModule({
    declarations: [...exports],
    imports: [
        IonicModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        FontAwesomeModule
    ],
    exports: [...exports],
})
export class FCModalSelectModule { }
