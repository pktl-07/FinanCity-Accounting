import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { Router } from '@angular/router';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { from } from 'rxjs';
import { MarketingItemService } from 'src/@sales/services/marketing-item.service';
import { PromotionItemDetailComponent } from '../promotion-item-detail/promotion-item-detail.component';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-promotion-item-list',
  templateUrl: './promotion-item-list.component.html',
  providers: [
    MarketingItemService,
    PWindowComponentService
  ]
})
export class PromotionItemListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true })
  _gridView: PGridViewComponent;

  private _subsink = new SubSink();

  constructor(
    public _router: Router,
    public _windowService: PWindowComponentService,
    public _promotionItemService: MarketingItemService) { }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => from(this.createNew({})) });

    this._gridView.config = {
      key: 'SysKey',
      fetch: this._promotionItemService,
      filterable: 'window',
      methodName: 'getPager',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'Description', filter: true, title: 'Promotion Item' },
        { dataType: 'currency', field: 'Price', filter: true, title: 'Price' },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active' },
      ]
    }
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'default';
      let window = this._windowService.open({ title: 'Promotion Item Setup', componentType: PromotionItemDetailComponent, state: state });
      let windowComponent = window.content.instance as PromotionItemDetailComponent;

      windowComponent.loadData({ SysKey: param.SysKey });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {

          this._gridView.refresh();
        })
      resolve(null);
    })
  }
}
