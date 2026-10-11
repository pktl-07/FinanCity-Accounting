import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ClientService } from 'src/app/services/client-service';
import { BehaviorSubject } from 'rxjs';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { BroadcasterService } from 'src/app/services/broadcaster.service';
import { FCFilterToolbarComponent } from 'src/@common/components/toolbar/fc-filter-toolbar.component';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-client-contact-list',
  templateUrl: './client-contact-list.page.html',
  providers: [
    ClientService,
    FilterConnectorService,
  ]
})
export class ClientContactListPage implements OnInit, OnDestroy {

  private _subsink = new SubSink();
  activeItems = new BehaviorSubject<any>([{ value: true, text: 'Yes' }, { value: false, text: 'No' }]);
  @ViewChild(FCFilterToolbarComponent, { static: true }) _filterTool: FCFilterToolbarComponent;

  constructor(private _router: Router,
    private _broadcaster: BroadcasterService,
    public _clientService: ClientService,
    public _filterConnectorService: FilterConnectorService) { }

  ngOnInit() {

    this._subsink.sink = this._broadcaster.on('client-reload')
      .subscribe(() => {
        this._filterTool.refresh();
      });
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  async handelViewDetail(item: any) {
    return await this._router.navigateByUrl(`/client-contact-detail/${item.SysKey}`);
  }

  async createNew() {
    return await this._router.navigateByUrl(`/client-contact-detail`);
  }
}
