import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { CountryService } from 'src/@sales/services/country.service';
import { PostalCodeService } from 'src/@common/service/postal-code.service';
import { finalize } from 'rxjs/operators';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-content-information',
  templateUrl: './content-information.component.html',
  providers: [CountryService, PostalCodeService]
})
export class ContentInformationComponent implements OnInit , OnDestroy {

  @Input() formGroup: FormGroup;
  loading: boolean = false;

  private _subsink = new SubSink();
  @Input() public showTelephoneNumber: boolean = true;
  @Input() public showMobileNumber: boolean = true;
  @Input() public showEmailAddress: boolean = true;
  @Input() public showWebSite: boolean = true;
  @Input() public isReadonly: boolean = false;
  @Input() public width: number = 6;

  constructor(
    public _country: CountryService, public _postalCodeService: PostalCodeService) {
  }

  ngOnDestroy(): void {
   this._subsink.unsubscribe();
  }

  filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  };

  get readonly(): boolean {
    return this.isReadonly == true ? true : null;
  }

  ngOnInit() {

    this._country.asyncFinds();
  }

  onChangePostalCode(value: string) {

    this.loading = true;
    this._subsink.sink = this._postalCodeService.findByKey(value)
      .pipe(
        finalize(() => this.loading = false))
        .subscribe(d => {

          if (d && d != null) {
            this.formGroup.get('BuildingHouseNo').setValue(d['Blk']);
            this.formGroup.get('Address1').setValue(d['Address1']);
            this.formGroup.get('Address2').setValue(d['Address2']);
          } else {
            this.formGroup.get('BuildingHouseNo').setValue(null);
            this.formGroup.get('Address1').setValue(null);
            this.formGroup.get('Address2').setValue(null);
          }
        })
  }
}
