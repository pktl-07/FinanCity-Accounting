import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { CommissionPayableService } from 'src/@sales/services/commission-payable.service';
import { PayableSchemeService } from 'src/@sales/services/payable-scheme.service';
import { CommissionReceivedAllocationService } from 'src/@sales/services/commission-received-allocation.service';
import { SalesInvoiceService } from 'src/@sales/services/sales-invoice.service';

@Component({
  selector: 'app-commission-payable-detail-detail',
  templateUrl: './commission-payable-detail-detail.component.html',
  providers: [
    CommissionPayableService,
    PayableSchemeService,
    CommissionReceivedAllocationService,
    SalesInvoiceService
  ]
})
export class CommissionPayableDetailDetailComponent {

  @Input() formGroup: FormGroup;
}
