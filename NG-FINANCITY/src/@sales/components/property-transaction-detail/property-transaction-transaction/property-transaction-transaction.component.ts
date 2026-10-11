import { Component, OnInit, Input, OnDestroy, Injector } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-property-transaction-transaction',
  templateUrl: './property-transaction-transaction.component.html',
})
export class PropertyTransactionTransactionComponent implements OnDestroy, OnInit {

  @Input() formGroup: FormGroup;
  @Input() contactFormGroup: FormGroup;

  private _subsink: SubSink;

  get readonly() {
    return this.formGroup.get('RecordStatus').value >= 6;
  }

  constructor() {
    this._subsink = new SubSink();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit(): void {

    this._subsink.sink = this.formGroup.get('ExclusiveListing').valueChanges
      .subscribe(d => {

        if (d) {
          this.formGroup.get('NotifyStartDate').enable();
          this.formGroup.get('NotifyEndDate').enable();
        } else {

          this.formGroup.get('NotifyStartDate').disable();
          this.formGroup.get('NotifyEndDate').disable();
          this.formGroup.get('NotifyStartDate').setValue(null);
          this.formGroup.get('NotifyEndDate').setValue(null);
        }
      })

    this.formGroup.get('ExclusiveListing').setValue(this.formGroup.get('ExclusiveListing').value);
  }
}
