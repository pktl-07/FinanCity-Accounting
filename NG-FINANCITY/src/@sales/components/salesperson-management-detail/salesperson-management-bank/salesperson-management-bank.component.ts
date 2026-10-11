import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-salesperson-management-bank',
  templateUrl: './salesperson-management-bank.component.html'
})
export class SalespersonManagementBankComponent {

  @Input() formGroup: FormGroup;

  constructor() {
  }
}
