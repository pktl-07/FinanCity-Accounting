import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { PropertyTransactionDetailPage } from './property-transaction-detail.page';
import { SuperTabsModule } from '@ionic-super-tabs/angular';
import { ContactPageModule } from '../shared/contact/contact.module';
import { FCValidatorModule } from 'src/@common/components/validator/fc-validator.module';
import { PropertyPage } from './property.page';
import { TNCPage } from './tnc.page';
import { CommissionPage } from './commission.page';
import { ClientPage } from './client.page';
import { ClientCreateModalPageModule } from '../shared/client-create-modal/client-create-modal.module';
import { SalespersonSalesPage } from './salesperson-sales.page';
import { SalespersonSalesModalModule } from '../shared/salesperson-sales-modal/salesperson-sales-modal.module';
import { PageUtilityService } from './page-utility.service';
import { ClientListModalModule } from '../shared/client-list-modal/client-list-modal.module';
import { ExternalSalespersonSalesPage } from './external-cobroke-sales.page';
import { ExternalSalespersonSalesModalModule } from '../shared/external-cobroke-sales-modal/external-cobroke-sales-modal.module';
import { BillToCoBrokePage } from './bill-to-cobroke.page';
import { CompanyListModalPageModule } from '../shared/company-list-modal/company-list-modal.module';
import { ExternalCobrokeListModalModule } from '../shared/external-cobroke-list-modal/external-cobroke-list-modal.module';
import { FCRecordStatusModule } from 'src/@common/components/status/fc-record-status.module';
import { FCCurrencyFormatModule, FCDateFormatModule, FCDecimalFormatModule } from 'src/@common/directives/fc-directive.module';
import { FCModalSelectModule } from 'src/@common/components/modal-select/modal-select.module';
import { FCImagePipeModule } from 'src/@common/pipes/fc.pipe.module';

@NgModule({
  declarations: [PropertyTransactionDetailPage,
    PropertyPage,
    TNCPage,
    CommissionPage,
    ClientPage,
    SalespersonSalesPage,
    ExternalSalespersonSalesPage,
    BillToCoBrokePage
  ],
  imports: [
    IonicModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SuperTabsModule,
    ContactPageModule,
    SalespersonSalesModalModule,
    FCValidatorModule,
    ClientCreateModalPageModule,
    ClientListModalModule,
    ExternalSalespersonSalesModalModule,
    CompanyListModalPageModule,
    ExternalCobrokeListModalModule,
    FCRecordStatusModule,
    FCDecimalFormatModule,
    FCCurrencyFormatModule,
    FCDateFormatModule,
    FCModalSelectModule,
    FCModalSelectModule,
    FCImagePipeModule,
    RouterModule.forChild([{ path: '', component: PropertyTransactionDetailPage }]),
  ],
  providers: [PageUtilityService]
})
export class PropertyTransactionDetailPageModule { }
