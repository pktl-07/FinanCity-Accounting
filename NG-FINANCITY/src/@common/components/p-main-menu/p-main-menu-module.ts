import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PMainMenuComponent } from './p-main-menu-component';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MenusModule } from "@progress/kendo-angular-menu";

@NgModule({
  declarations: [PMainMenuComponent],
  imports: [
    CommonModule,
    BrowserModule,
    BrowserAnimationsModule,
    ButtonModule,
    MenusModule
  ],
  exports: [PMainMenuComponent]
})
export class PMainMenuModule { }
