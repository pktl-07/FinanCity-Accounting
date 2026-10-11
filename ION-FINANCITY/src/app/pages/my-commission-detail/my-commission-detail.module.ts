import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { MyCommissiontDetailPage } from './my-commission-detail.page';
import { FCRecordStatusModule } from 'src/@common/components/status/fc-record-status.module';
import { FCCurrencyFormatModule, FCDateFormatModule } from 'src/@common/directives/fc-directive.module';

@NgModule({
  declarations: [MyCommissiontDetailPage,],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCRecordStatusModule,
    FCCurrencyFormatModule,
    FCDateFormatModule,
    RouterModule.forChild([{ path: '', component: MyCommissiontDetailPage }]),
  ]
})
export class MyCommissionDetailPageModule { }
