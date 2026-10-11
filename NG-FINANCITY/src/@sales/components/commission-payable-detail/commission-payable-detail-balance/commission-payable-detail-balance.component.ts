import { Component, OnDestroy, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { map } from 'rxjs/operators';
import { CommissionPayableService } from 'src/@sales/services/commission-payable.service';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';
import { PageUtilityService } from '../page.utility.service';

@Component({
  selector: 'app-commission-payable-detail-balance',
  templateUrl: './commission-payable-detail-balance.component.html',
  providers: [
    CommissionPayableService,
  ]
})
export class CommissionPayableDetailBalanceComponent implements OnDestroy {

  @Input() formGroup: FormGroup;

  public _subsink = new SubSink();

  constructor(
    private _pageUtilityService: PageUtilityService,
    private _commissionPayableService: CommissionPayableService) {

    this._subsink.sink = this._pageUtilityService.statusChangeSubject
      .subscribe(() => {

        let associateComm = this.formGroup.get('CommissionPayableToSalesperson').value;
        this._subsink.sink = this._commissionPayableService.finds({
          filter: {
            filters: kendoFilter([
              { field: 'ALOSK', value: this.formGroup.get('ALOSK').value, operator: 'eq', logic: 'and' },
              { field: 'RecordStatus', value: 8, operator: 'eq', logic: 'and' }
            ]),
            logic: 'and'
          }
        }).pipe(
          map((paid: []) => {
            return {
              paid: paid.reduce((sum, current) => sum + current['NetPaid'], 0)
            }
          }),
        ).subscribe((data: any) => {

          this.formGroup.get('CommissionPaid').patchValue(data['paid']);
          this.formGroup.get('Balance').patchValue(associateComm - data['paid']);
        });
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }
}
