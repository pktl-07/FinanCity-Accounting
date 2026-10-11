import { Component, OnDestroy, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { FormGroup } from '@angular/forms';
import { BankBestRateService } from 'src/app/services/bank-best-rate.service';
import { logoPicture, SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-bank-best-rate',
  templateUrl: './bank-best-rate.page.html',
  providers: [
    BankBestRateService
  ]
})
export class BankBestRatePage implements OnInit, OnDestroy {

  public form: FormGroup;
  public activeItems = new BehaviorSubject<any>([{ value: true, text: 'Yes' }, { value: false, text: 'No' }]);
  private _subsink = new SubSink();

  constructor(
    public _bankBestRateService: BankBestRateService) {
  }

  ngOnInit() {

    this._bankBestRateService.asyncFinds();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  getPicture(key: string) {

    return logoPicture[key];
  }
}
