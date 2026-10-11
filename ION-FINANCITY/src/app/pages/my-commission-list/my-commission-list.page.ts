import { Component, ViewChild } from '@angular/core';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { Router } from '@angular/router';
import { FCFilterToolbarComponent } from 'src/@common/components/toolbar/fc-filter-toolbar.component';
import { DataListService } from 'src/app/services/data-list.service';

@Component({
  selector: 'fc-my-commission-list',
  templateUrl: './my-commission-list.page.html',
  providers: [
    DataListService,
    FilterConnectorService
  ]
})
export class MyCommissionListPage {

  @ViewChild(FCFilterToolbarComponent, { static: true }) _filterTool: FCFilterToolbarComponent;
  status = [
    { value: 1, text: 'Pending' },
    { value: 2, text: 'Paid to You' }];

  constructor(
    public _dataListService: DataListService,
    private _router: Router,
    public _filterConnectorService: FilterConnectorService) {
  }

  async onViewDetail(data: any) {
    return await this._router.navigateByUrl(`/my-commission-detail/${data.PropertySK}`);
  }
}
