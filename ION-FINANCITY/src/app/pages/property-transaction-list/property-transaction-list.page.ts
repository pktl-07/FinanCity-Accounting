import { Component, ViewChild, Inject } from '@angular/core';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { Subject } from 'rxjs';
import { FCFilterToolbarComponent } from 'src/@common/components/toolbar/fc-filter-toolbar.component';
import { FormBuilder, FormGroup } from '@angular/forms';
import { FCPagerContainerComponent } from 'src/@common/components/pager/fc-pager-container-component';

@Component({
  selector: 'fc-property-transaction-list',
  templateUrl: './property-transaction-list.page.html',
  providers: [
    FilterConnectorService,
    { provide: 'morgage-connector', useClass: FilterConnectorService },
    { provide: 'referral-connector', useClass: FilterConnectorService }
  ]
})
export class PropertyTransactionListPage {

  @ViewChild('mortgagePagerContainer', { read: FCPagerContainerComponent }) _mortgagePagerContainer: FCPagerContainerComponent;
  @ViewChild('mortgageFilter', { read: FCFilterToolbarComponent }) _mortgageFilter: FCFilterToolbarComponent;

  recordInfo = new Subject<any>();
  form: FormGroup;

  constructor(
    _formBuilder: FormBuilder,
    @Inject('morgage-connector') public morgageConnector: FilterConnectorService,
    @Inject('referral-connector') public referralConnector: FilterConnectorService) {

    this.form = _formBuilder.group({
      activeSegment: 1
    })
  }
}
