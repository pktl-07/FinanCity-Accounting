import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { accountingModules, SubSink } from 'src/@common/service/utility-service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-journal-ledger-list.component',
  templateUrl: './journal-ledger-list.component.html',
  providers: [
  ]
})
export class JournalLedgerListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;

  private _subsink = new SubSink();
  modules = accountingModules();
  currentAccountingYear = null;

  constructor(
    public _router: Router,
    public _activatedRoute: ActivatedRoute) {
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this._router.navigate([`Accounting/JournalLedgerDetail`]) });

    this._subsink.sink = this._activatedRoute.data
      .subscribe((data) => {
        this.currentAccountingYear = data['data'];
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }
}
