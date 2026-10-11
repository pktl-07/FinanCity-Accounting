import { Component, OnInit, OnDestroy, ViewChild, ViewChildren, QueryList } from '@angular/core';
import { AccountSetupService } from 'src/@accounts/services/account-setup.service';
import { AccountCategoryService } from 'src/@accounts/services/account-category.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { AccountSetupDetailComponent } from '../account-setup-detail/account-setup-detail.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { Router } from '@angular/router';
import { TabStripComponent } from '@progress/kendo-angular-layout';
import { ChartOfAccountGridComponent } from './chart-of-accounts-grid.component';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-chart-of-accounts',
  templateUrl: './chart-of-accounts.component.html',
  providers: [
    AccountSetupService,
    AccountCategoryService,
    PWindowComponentService
  ]
})
export class ChartOfAccountComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;
  @ViewChild(TabStripComponent, { static: true }) _tabStrip: TabStripComponent;
  @ViewChildren(ChartOfAccountGridComponent) _gridQueryList: QueryList<ChartOfAccountGridComponent>;

  private _subsink = new SubSink();

  accounts = [
    {
      value: '1', text: 'Asset'
    }, {
      value: '2', text: 'Liability'
    }, {
      value: '3', text: 'Equity'
    }, {
      value: '4', text: 'Income'
    }, {
      value: '5', text: 'Cost of Sales'
    }, {
      value: '6', text: 'Expense'
    }, {
      value: '8', text: 'Other Income'
    }, {
      value: '9', text: 'Other Expense'
    },
  ]

  constructor(public _windowService: PWindowComponentService,
    public _router: Router,) {
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.handleDetail({}) });

    this.selectedTab = this.accounts[0];
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  handleDetail(param: { AccountSK?: string }) {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Account Setup', componentType: AccountSetupDetailComponent, state: state });
    let windowComponent = window.content.instance as AccountSetupDetailComponent;

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe((data) => {

        this._gridQueryList.find(f => f.id == this.selectedTab.value).refresh();
      })

    return param.AccountSK ? windowComponent.loadData({ SysKey: param.AccountSK }) : windowComponent.loadData({});
  }

  selectedTab: { text: string, value: string };
  handleSelect(tab: any) {

    this.selectedTab = this.accounts.find(f => f.text == tab.title);
  }
}
