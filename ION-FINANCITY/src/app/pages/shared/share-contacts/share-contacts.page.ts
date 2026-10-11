import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { SubSink } from 'src/@common/utils';
import { SalespersonManagementService } from 'src/app/services/salesperson-management.service';
import { ShareContactService } from './share-contacts.service';

export interface SelectionItemModel {
  value: string,
  checked: boolean,
  name: string,
  show: boolean,
}

@Component({
  selector: 'fc-share-contacts',
  templateUrl: './share-contacts.page.html',
  providers: [
    SalespersonManagementService,
    ShareContactService
  ]
})
export class ShareContactsPage implements OnInit, OnDestroy {

  @Input() keySelection = [];

  private _subsink = new SubSink();
  private _cloneOriginKeys = [];

  constructor(public _salespersonManagementService: SalespersonManagementService,
    private _shareContactService: ShareContactService) {
  }

  indeterminateState = false;
  checkParent = false;
  checkItems: SelectionItemModel[] = [];

  ngOnInit() {

    this._cloneOriginKeys.push(...this.keySelection);
    this._subsink.sink = this._shareContactService
      .originSubject
      .subscribe((value) => {
        if (value != null) {
          value.forEach((data) => {
            this.checkItems.push({
              value: data.SysKey,
              name: data.Name,
              show: true,
              checked: this.keySelection.includes(data.SysKey)
            })
          })
          this.handleCheckState();
        }
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  handleCheckState() {

    if (typeof this._shareContactService.salespersonClone != 'undefined'
      && this._shareContactService.salespersonClone != null) {

      //this.keySelection = [];
      const allItems = this._shareContactService.salespersonClone.length;
      let selected = 0;

      this.checkItems.map(item => { if (item.checked) selected++ })

      if (selected > 0 && selected < allItems) {
        // One item is selected among all checkbox elements
        this.indeterminateState = true;
        this.checkParent = false;
      } else if (selected > 0 && selected == allItems) {
        // All item selected
        this.checkParent = true;
        this.indeterminateState = false;
      }
      else if (selected == 0) {
        //un selected all..
        this.checkParent = false;
        this.indeterminateState = false;
      }
    }
  }

  handleSelectAll() {

    setTimeout(() => {
      this.checkItems.forEach(item => {
        item.checked = this.checkParent;
      })
    })
  }

  getInsertedItem() {

    const comparekeys = this.getSelecetedKeys();
    return this._shareContactService.getInsertedItem({
      originKeys: this._cloneOriginKeys,
      selectedKeys: comparekeys
    });
  }

  getDeletedItem() {

    const comparekeys = this.getSelecetedKeys();
    return this._shareContactService.getDeletedItem({
      originKeys: this._cloneOriginKeys,
      selectedKeys: comparekeys
    });
  }

  getSelecetedKeys() {

    return this.checkItems.filter(x => x.checked == true).map(m => m.value);
  }

  reBindKeyCollection(keys: string[]) {

    this._cloneOriginKeys = [];
    this.checkItems = [];
    this.keySelection = [];

    this._cloneOriginKeys.push(...keys);
    this.keySelection.push(...keys);
    this._shareContactService.poppulateSalespersonList();
  }

  handleSearchInput(event: any) {

    const value = <string>event.target.value;
    if (value != '') {
      this.checkItems.filter(f => !f.name.toLowerCase().includes(value.toLowerCase()))
        .map((data) => data.show = false)
    } else {
      this.checkItems.map((data) => data.show = true)
    }
  }
}
