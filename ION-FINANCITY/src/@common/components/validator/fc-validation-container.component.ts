import { Component, Input, AfterContentInit, ContentChildren, QueryList } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { FCValidatorBase } from './fc-validator-base';

@Component({
  selector: 'fc-validation-container',
  template: `
      <div *ngIf="control?.invalid && (control?.dirty || control?.touched)">
        <span class="error-message" *ngFor="let validator of validators">
          <ng-container *ngIf="validator?.control?.errors[validator.error]">
            <ion-icon name="information-circle-outline"></ion-icon> '{{ validator.label }}' {{ validator.message }}
          </ng-container>
        </span>
      </div>      
  `
})
export class FcValidationContainerComponent implements AfterContentInit {

  @Input() public control: AbstractControl;
  @Input() public label: string;

  @ContentChildren(FCValidatorBase) validators: QueryList<FCValidatorBase>;

  ngAfterContentInit(): void {

    this.validators.map((d, i) => {
      d.control = this.control;
      d.label = this.label;
    })
  }
}
