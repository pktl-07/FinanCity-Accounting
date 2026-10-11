import { Component, OnInit, Input, OnDestroy, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { TNC_TransactionTypeService } from '../../../services/tnc_transactionType.serivce';
import { TNC_PropertyTypeService } from '../../../services/tnc_propertyType.service';
import { TNC_PropertyPurposeService } from '../../../services/tnc_propertyPurpose.service';
import { TNC_InterestRateTypeService } from '../../../services/tnc_interestRateType.service';
import { TNC_RateTypeService } from '../../../services/tnc_rateType.service';
import { TNC_DurationService } from '../../../services/tnc_duration.service';
import { TNC_TimeFrameService } from '../../../services/tnc_timeFrame.service';
import { PromotionItemExpenseComponent } from '../promotion-item-expense/promotion-item-expense.component';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-property-transaction-tnc',
  templateUrl: './property-transaction-tnc.component.html',
  providers: [
    TNC_TransactionTypeService,
    TNC_PropertyTypeService,
    TNC_PropertyPurposeService,
    TNC_InterestRateTypeService,
    TNC_RateTypeService,
    TNC_DurationService,
    TNC_TimeFrameService,
    PWindowComponentService]
})
export class PropertyTransactionTNCComponent implements OnDestroy, OnInit {

  @ViewChild('expenseComponent') expenseComponent: PromotionItemExpenseComponent;
  @Input() formGroup: FormGroup;

  get readonly() {
    return this.formGroup.get('RecordStatus').value >= 6;
  }

  private _subsink = new SubSink();
  public _transactoinType: Array<any> = new Array<any>();
  public _propertyPurpose: Array<any> = new Array<any>();
  public _interestRateType: Array<any> = new Array<any>();
  public _rateType: Array<any> = new Array<any>();
  public _propertyType: Array<any> = new Array<any>();
  public _defaultItem: any = { SysKey: null, Description: "N/A" };

  companyDefaultItem: any = { SysKey: null, CompanyName: 'N/A' };

  constructor(public _windowService: PWindowComponentService,
    public _tncTransactionTypeSerivce: TNC_TransactionTypeService,
    public _tncPropertyPurposeService: TNC_PropertyPurposeService,
    public _tncInterestRateTypeService: TNC_InterestRateTypeService,
    public _tncPropertyTypeService: TNC_PropertyTypeService,
    public _tncDurationService: TNC_DurationService,
    public _tncRateTypeService: TNC_RateTypeService,
    public _tncTimeFrameService: TNC_TimeFrameService) {
  }

  ngOnInit(): void {

    this._tncDurationService.asyncFinds();
    this._tncTimeFrameService.asyncFinds();

    this._subsink.sink = this._tncInterestRateTypeService.finds().subscribe(data => {
      this._interestRateType = data;
    });

    this._subsink.sink = this._tncRateTypeService.finds().subscribe(data => {
      this._rateType = data;
    });

    this._subsink.sink = this._tncTransactionTypeSerivce.finds().subscribe(data => {
      this._transactoinType = data;
    });

    this._subsink.sink = this._tncPropertyTypeService.finds().subscribe(data => {
      this._propertyType = data;
    });

    this._subsink.sink = this._tncPropertyPurposeService.finds().subscribe(data => {
      this._propertyPurpose = data;
    });

    this.disableTransactionType(this.formGroup.get('TransactionTypeSK').value);
    this._subsink.sink = this.formGroup.get('TransactionTypeSK').valueChanges
      .subscribe((data: any) => {
        this.disableTransactionType(data);
      });
  }

  private disableTransactionType(key: string) {

    if (key == '2017042514462609136')
      this.formGroup.get('TransactionTypeCompleteStatus').setValue(false);
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  };
}
