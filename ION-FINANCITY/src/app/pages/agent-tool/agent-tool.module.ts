import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Routes, RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { AgentToolPage } from './agent-tool.page';
import { SuperTabsModule } from '@ionic-super-tabs/angular'
import { FinantialCalculatorPageModule } from './financial-calculator/financial-calculator.module';
import { BankBestRatePageModule } from './bank-best-rate/bank-best-rate.module';
import { FCNotificationComponentModule } from '../shared/notification/notification-component-module';

const routes: Routes = [
  {
    path: '',
    component: AgentToolPage
  }
];

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    RouterModule.forChild(routes),
    SuperTabsModule,
    FinantialCalculatorPageModule,
    BankBestRatePageModule,
    FCNotificationComponentModule
  ],
  declarations: [AgentToolPage]
})
export class AgentToolPageModule { }
