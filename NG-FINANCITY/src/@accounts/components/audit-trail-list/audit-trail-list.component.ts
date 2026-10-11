import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { ActivatedRoute, Router } from '@angular/router';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-audit-trail-list.component',
  templateUrl: './audit-trail-list.component.html',
  providers: [
  ]
})
export class AuditTrailListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;

  private _subsink = new SubSink();
  currentAccountingYear = null;

  constructor(
    public _router: Router,
    public _activatedRoute: ActivatedRoute) {
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') });

    this._subsink.sink = this._activatedRoute.data
      .subscribe((data) => {
        this.currentAccountingYear = data['data'];
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }
}
