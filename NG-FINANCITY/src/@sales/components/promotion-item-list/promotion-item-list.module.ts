import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PromotionItemListComponent } from './promotion-item-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { PromotionItemDetailModule } from '../promotion-item-detail/promotion-item-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [PromotionItemListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    PromotionItemDetailModule,
    RouterModule.forChild([{ path: '', component: PromotionItemListComponent }])
  ]
})
export class PromotionItemListModule { }
