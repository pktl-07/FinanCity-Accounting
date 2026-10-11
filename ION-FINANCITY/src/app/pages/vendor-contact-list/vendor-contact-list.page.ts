import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { BroadcasterService } from 'src/app/services/broadcaster.service';
import { FCFilterToolbarComponent } from 'src/@common/components/toolbar/fc-filter-toolbar.component';
import { VendorType, ActiveSource } from 'src/@common/common.constant';
import { VendorService } from 'src/app/services/vendor.service';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-cvendor-contact-list',
  templateUrl: './vendor-contact-list.page.html',
  providers: [
    VendorService,
    FilterConnectorService]
})
export class VendortListPage implements OnInit, OnDestroy {

  private _subsink = new SubSink();
  activeItems = new BehaviorSubject<any>(ActiveSource);
  vendorType = new BehaviorSubject<any>(VendorType);

  @ViewChild(FCFilterToolbarComponent, { static: true }) _filterTool: FCFilterToolbarComponent;

  constructor(private _router: Router,
    private _broadcaster: BroadcasterService,
    public _vendorService: VendorService,
    public _filterConnectorService: FilterConnectorService) { }

  ngOnInit() {

    this._subsink.sink = this._broadcaster.on('vendor-list-reload')
      .subscribe(() => {
        this._filterTool.refresh();
      });
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  async handelViewDetail(item: any) {
    return await this._router.navigateByUrl(`/vendor-contact-detail/${item.SysKey}`);
  }

  async createNew() {
    return await this._router.navigateByUrl(`/vendor-contact-detail`);
  }
}
