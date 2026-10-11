import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { Router } from '@angular/router';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { DetailView } from 'src/@common/Infrastructure/common-interface';
import { LeadService } from 'src/@sales/services/lead.service';

@Component({
  selector: 'app-lead-management-salesperson-list',
  template: '<p-grid-view #salesperson></p-grid-view>',
  providers: [
    LeadService,
  ]
})
export class LeadManagementSalespersonListComponent implements OnInit, DetailView {

  @ViewChild('salesperson', { static: true }) public _gridView: PGridViewComponent;
  @Input('dataItem') public dataItem: any;

  constructor(
    public _router: Router,
    public _leadService: LeadService) {
  }

  ngOnInit() {

    this._gridView.config = {
      key: 'LeadSK',
      methodName: 'asyncLeadShare',
      fetch: this._leadService,
      filterable: 'none',
      columns: [
        { dataType: 'hidden', field: 'LeadSK', filter: true, title: '', filterOptions: { defaultValue1: this.dataItem.SysKey } },
        { dataType: 'string', field: 'Name', filter: false, title: 'Salesperson' },
        { dataType: 'string', field: 'NRIC', filter: false, title: 'NRIC', width: 120 },
        { dataType: 'checkbox', field: 'IsActive', filter: false, title: 'Is Active' },
      ]
    };
  }
}
