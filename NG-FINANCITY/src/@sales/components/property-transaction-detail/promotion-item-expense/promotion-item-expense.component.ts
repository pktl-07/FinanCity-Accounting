import { Component, OnInit, Input, ViewChild, OnDestroy } from '@angular/core';
import { PGridIncellEditingComponent } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { Validators, FormGroup } from '@angular/forms';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PageUtilityService } from '../page.utility.service';
import { MarketingItemService } from 'src/@sales/services/marketing-item.service';
import { PromotionItemDetailComponent } from '../../promotion-item-detail/promotion-item-detail.component';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-promotion-item-expense',
  templateUrl: './promotion-item-expense.component.html',
  providers: [
    MarketingItemService,
    PWindowComponentService
  ]
})
export class PromotionItemExpenseComponent implements OnDestroy, OnInit {

  @ViewChild('promotionItemExpense', { read: PGridIncellEditingComponent, static: true }) public dataGrid: PGridIncellEditingComponent;
  @Input() formGroup: FormGroup;
  private _subsink = new SubSink();

  get readonly() {
    return this.formGroup.get('RecordStatus').value >= 6;
  }

  constructor(public _windowService: PWindowComponentService,
    private _pageUtilityService: PageUtilityService,
    public _promotionItemService: MarketingItemService) {
  }

  ngOnInit(): void {

    this._init();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe()
  }

  public _init() {

    this._promotionItemService.asyncFinds();
    this.dataGrid.config = {
      key: 'SysKey',
      fetch: this._pageUtilityService.promotionItemExpenseSubject,
      columns: [
        { dataType: 'hidden', field: 'SysKey', defaultValue: null },
        { dataType: 'hidden', field: 'PTOSK', defaultValue: null },
        {
          dataType: 'dropdown', field: 'ItemSK', title: 'Item'
          , validators: Validators.required
          , dropdown: {
            fetch: this._promotionItemService,
            textField: 'Description',
            valueField: 'SysKey',
            disabledItem: (item) => !item.IsActive,
            itemEmpty: (i) => this.addNewPromotionItem(i)
          },
          cellValueChange: (c) => {

            this._subsink.sink = this._promotionItemService.findByKey(c.event)
              .subscribe(d => {
                c.fg.get('Amount').setValue(d.Price);
              })
          }
        },
        { dataType: 'currency', field: 'Amount', title: 'Amount', width: 120, defaultValue: 0, validators: [Validators.required, Validators.min(0)] },
      ],
    }
  }

  addNewPromotionItem(i?: (j: any) => any) {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Promotion Item Setup', componentType: PromotionItemDetailComponent, state: state });
    let windowComponent = window.content.instance as PromotionItemDetailComponent;

    windowComponent.loadData({ Description: this.dataGrid.dropDownFilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {
        if (i) {
          i(item.data.SysKey);
        }
        this._windowService.close();
      });
  }
}
