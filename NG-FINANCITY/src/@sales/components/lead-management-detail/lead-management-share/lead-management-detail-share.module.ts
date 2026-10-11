import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { LeadManagementDetailShareComponent } from './lead-management-detail-share.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { TextBoxModule } from '@progress/kendo-angular-inputs';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PSalespersonPipeModule } from 'src/@common/pipe/p-salesperson-pipe/p-salesperson-pipe-module';

const components = [LeadManagementDetailShareComponent];

@NgModule({
    entryComponents: [components],
    declarations: [components],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        GridModule,
        TextBoxModule,
        ButtonModule,
        PSalespersonPipeModule,
    ],
    exports: [components]
})
export class LeadManagementDetailShareModule { }
