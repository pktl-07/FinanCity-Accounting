import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
//import * as $ from 'jquery';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: [`

    .fx-formObject {
        display: table;
        margin-bottom: 12px;
        min-height: 10px;
        border: 0;
        border-spacing: 12px 0;
    }

    .fx-menu-group{
      margin-top:12px;
    }

    .fx-menu-title{
      font-size:22px;
      margin-bottom:12px
    }

    .fx-main-menu .fx-menu-wrap .fx-menu-group .fx-menu-item-wrap{
      margin-left:40px;
      overflow:auto
    }

    .fx-main-menu .fx-menu-wrap .fx-menu-group .fx-menu-item-wrap a span.title{
      left:0;
      bottom:8px
    }

    .fx-main-menu .fx-menu-wrap .fx-menu-group .fx-menu-item-wrap a.half span.icon span {
      margin-left: 20px;
    }

    .fx-main-menu .fx-menu-wrap .fx-menu-group .fx-menu-item-wrap a span.icon{
      top:0;
      left:0;
      width:100%;
      height:100%
    }  

    .fx-main-menu .fx-menu-wrap .fx-menu-group .fx-menu-item-wrap a span.icon span{
        display: block;
        margin-left: 85px;
        margin-top: 8px;
        width: 64px;
        height: 64px;
        font-family: Flaticon;
        color: #FFF;
        line-height: 62px;
        font-weight: 400;
        text-indent: 0;
        text-align: center;
        font-size: 40px!important;
        border: 3px solid transparent;
        border-radius: 50%;
        -webkit-border-redius: 50%;
        -moz-border-redius: 50%;
        -ms-border-redius: 50%;
    }

    .fx-main-menu .fx-menu-wrap .fx-menu-group .fx-menu-item-wrap a{
      display: block;
      position: relative;
      float: left;
      margin-left: 12px;
      margin-bottom: 12px;
      width: 237px;
      height: 112px;
      background-color: #00aeef;
      border: 4px solid transparent;
      color: #FFF;
      font-weight: 700;
      text-indent: 10px;
      user-select: none;
      -moz-user-select: none;
      -ms-user-select: none;
      -webkit-user-select: none;
      -webkit-user-drag: none;
      cursor: pointer;
    }

    .fx-main-menu .fx-menu-wrap .fx-menu-group .fx-menu-item-wrap a.half {
      width: 112px;
    }

    a:-webkit-any-link {
      color: -webkit-link;
      cursor: pointer;
      text-decoration: underline;
    }

    .fx-main-menu .fx-menu-wrap .fx-menu-group .fx-menu-item-wrap a span {
      position: absolute;
    }

    .fx-menu-group:nth-child(1) .fx-menu-item-wrap a:nth-child(1) {
      background-color: #648e05;
    }
    
  `]
})
export class HomeComponent implements OnInit {

  private _items: any;
  public business: any;
  public management: any;
  public configuration: any;
  public users: any;
  public accounting: any;
  public cashbook: any;

  constructor(public _router: Router) {
  }

  ngOnInit(): void {

    this._items = this.mapItems(this._router.config);
    this.business = this.filterTraversal('Business');
    this.accounting = this.filterTraversal('Accounting');
    this.management = this.filterTraversal('Management');
    this.configuration = this.filterTraversal('Setup');
    this.cashbook = this.filterTraversal('Cashbook');
    this.users = this.filterTraversal('Users');
  }

  handleNavigate(path: string) {
    this._router.navigateByUrl(path);
  }

  public maxLength(text: string) {

    return text.length <= 9;
  }

  public onTagMouseOver(target: HTMLSpanElement, item: any) {

    let a = $(target).closest('a').first();
    if (!(item.borderColor as string).toLowerCase().startsWith('solid')) {
      a.css({ 'border-color': `var(--solid-${item.borderColor})` })
    } else {

      let i: string = (item.borderColor as string);
      let c = i.substring(6, i.length);
      a.css({ 'border-color': `var(--${c})` })
    }
  }

  public onTagMouseOut(target: HTMLSpanElement, item: any) {

    let a = $(target).closest('a').first();
    a.css({ 'border-color': `var(--${item.borderColor})` });
  }

  private mapItems(routes: any[], path?: string): any[] {

    return routes.filter(x => (x.display && x.display) != 'none')
      .map(item => {
        const result: any = {
          text: item.text,
          path: (path ? `${path}/` : '') + (item.outlet ? `(${item.outlet}:${item.path})` : item.path),
          display: item.display ? item.display : 'display',
          color: item.color,
          icon: item.icon,
        };

        if (item.children)
          result.items = this.mapItems(item.children, result.path);

        return result;
      })
  }

  private filterTraversal(title: string) {

    let f: any = (this._items as Array<any>)
      .filter(f => (f.text as string).toLowerCase().indexOf(title.toLowerCase()) != -1);

    let traversal = new Array<any>();
    this.traverse(f, traversal);

    return traversal;
  }

  private traverse(items: Array<any>, _traversal: Array<any>) {

    items.forEach(item => {
      if (item.items) {
        this.traverse(item.items, _traversal);
      } else {

        if (item.display != 'none') {
          let icon = {};
          icon[`${item.icon}`] = true;
          icon['gi-2x'] = true;

          _traversal.push({
            path: item.path,
            text: item.text,
            color: item.color ? { 'background-color': `var(--${item.color})` } : { 'background-color': `var(--blue)` },
            borderColor: item.color,
            icon: item.icon ? icon : { 'fab': true, 'gear': 'true', 'gi-2x': true }
          })
        }
      }
    })

    return _traversal;
  }
}
