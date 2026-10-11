import { Component, OnInit, ViewChild } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { Router } from '@angular/router';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { BranchOutService } from 'src/@sales/services/branch-out.service';

@Component({
  selector: 'app-branch-out-list',
  templateUrl: './branch-out-list.component.html',
  providers: [
    BranchOutService
  ]
})
export class BranchOutListComponent implements OnInit {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  constructor(
    public _branchOutService: BranchOutService,
    public _router: Router) {
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.detail({}) });

    this._gridView.config = {
      key: 'SysKey',
      methodName: 'getPager',
      fetch: this._branchOutService,
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.detail({ SysKey: a }) },
        { dataType: 'date', field: 'CreateDate', filter: true, title: 'Create Date' },
        { dataType: 'date', field: 'EffectiveDate', filter: true, title: 'Effective Date' },
        { dataType: 'string', field: 'SalepersonName', filter: true, title: 'Salesperson Name' },
        { dataType: 'string', field: 'NRIC', filter: true, width: 250, title: 'NRIC No.' },
        { dataType: 'string', field: 'From', filter: true, title: 'From', width: 120 },
        { dataType: 'string', field: 'To', filter: true, title: 'To', width: 120 },
        { dataType: 'string', field: 'ApprovedBy', filter: true, width: 250, title: 'Approved By' },
        { dataType: 'string', field: 'Remarks', filter: true, width: 250, title: 'Remarks' },
      ]
    }
  }

  detail(param: { SysKey?: string }) {

    if (param.SysKey)
      return this._router.navigate([`Management/SalespersonManagement/BranchOutDetail/${param.SysKey}`]);
    else
      return this._router.navigate([`Management/SalespersonManagement/BranchOutDetail`]);
  }
}
