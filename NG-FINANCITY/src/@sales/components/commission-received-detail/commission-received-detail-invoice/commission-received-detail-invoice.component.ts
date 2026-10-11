import { Component, Input, OnDestroy } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { PageUtilityService } from '../page.utility.service';
import { map } from 'rxjs/operators';
import { CommissionReceivedService } from 'src/@sales/services/commission-received.service';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-commission-received-detail-invoice',
  templateUrl: './commission-received-detail-invoice.component.html',
  providers: [
    CommissionReceivedService
  ]
})
export class CommissionReceivedDetailInvoiceComponent implements OnDestroy {

  @Input() public formGroup: FormGroup;

  private _subsink = new SubSink();

  constructor(private _pageUtilityService: PageUtilityService,
    private _commmissionReceivedService: CommissionReceivedService) {

    this._subsink.sink = this._pageUtilityService.receivedBalanceChangeSubject
      .subscribe(() => {

        this._subsink.sink = this._commmissionReceivedService.pager({
          skip: 0,
          take: 1,
          filter: {
            filters: kendoFilter([
              { field: 'InvoiceSK', value: this.formGroup.get('InvoiceSK').value, operator: 'eq', logic: 'and' }]),
            logic: 'and'
          }
        }).pipe(map(data => data.data[0]))
          .subscribe(data => {
            this.formGroup.get('TotalReceived').patchValue(data['TotalReceived']);
            this.formGroup.get('Balance').patchValue(data['Balance']);
          })
      });
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }
}
