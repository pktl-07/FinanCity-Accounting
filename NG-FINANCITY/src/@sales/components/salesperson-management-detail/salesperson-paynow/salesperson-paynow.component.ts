import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-salesperson-paynow',
  templateUrl: './salesperson-paynow.component.html'
})
export class SalespersonManagementPayNowComponent {

  @Input() formGroup: FormGroup;

  constructor() {
  }
}
