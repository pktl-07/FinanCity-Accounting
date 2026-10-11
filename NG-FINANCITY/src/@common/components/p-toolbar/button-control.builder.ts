import { Injectable } from '@angular/core';
import { IActionAnimationConfig, IDataComplete, IDataReady } from '../../Infrastructure/common-interface';
import { PToolbarComponent } from './p-toolbar.component';
import { PModalFormComponent } from '../p-modal-form/p-modal-form.component';
import { Subject, BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ButtonControlBuilder {

    public _control: PToolbarComponent | PModalFormComponent;

    Start(config: Subject<boolean>) {

        this._control.config.start = config;
        return this;
    }

    Success(config: Subject<IDataComplete>) {

        this._control.config.success = config;
        return this;
    }

    Ready(config: BehaviorSubject<IDataReady>) {

        this._control.config.fromReady = config;
        return this;
    }

    StateKey(config: string[]) {

        this._control.config.statekey = config;
        return this;
    }

    NavigateNew(config: IActionAnimationConfig) {

        this._control.config.buttons.push({ label: "navigate-new", action: config, icon: { 'fal': true, 'fa-plus': true }, orderyBy: 0 });
        return this;
    }

    NavigationButton(config: IActionAnimationConfig) {

        this._control.config.buttons.push({ label: "navigate", action: config });
        return this;
    }

    NewButton(config: IActionAnimationConfig) {

        config.requireConfirmation = true; //put default value

        this._control.config.buttons.push({ label: "New", action: config, icon: { 'fal': true, 'fa-plus': true }, orderyBy: 0 });
        config.validate = () => true;
        return this;
    }

    ApproveButton(config: IActionAnimationConfig) {

        config.requireConfirmation = true; //put default value
        config.requireNotification = true; //put default value

        this._control.config.buttons.push({ label: "Approve", action: config, icon: { 'far': true, 'fa-thumbs-up': true }, hide: config.disabled, orderyBy: 0.1 });
        return this;
    }

    ExportToPdf(config: IActionAnimationConfig) {

        config.requireConfirmation = false; //put default value
        config.requireNotification = false; //put default value
        config.requireLoading = true;

        this._control.config.buttons.push({ label: "Download Pdf", action: config, icon: { 'fas': true, 'fa-download': true }, hide: config.disabled, orderyBy: 0.4 });
        return this;
    }

    ExportToExcel(config: IActionAnimationConfig) {

        config.requireConfirmation = false; //put default value
        config.requireNotification = false; //put default value
        config.requireLoading = true;

        this._control.config.buttons.push({ label: "Download CVS", action: config, icon: { 'fas': true, 'fa-download': true }, hide: config.disabled, orderyBy: 0.5 });
        return this;
    }

    RejectButton(config: IActionAnimationConfig) {

        config.requireConfirmation = true; //put default value
        config.requireNotification = true; //put default value

        this._control.config.buttons.push({ label: "Reject", action: config, icon: { 'far': true, 'fa-thumbs-down': true }, hide: config.disabled, orderyBy: 0.2 });
        return this;
    }

    SaveButton(config: IActionAnimationConfig) {

        config.requireLoading = true;
        config.requireConfirmation = true; //put default value
        config.requireNotification = true; //put default value

        this._control.config.buttons.push({ label: "Save", action: config, icon: { 'fal': true, 'fa-paperclip': true }, hide: config.disabled, orderyBy: 1 });
        return this;
    }

    UpdateButton(config: IActionAnimationConfig) {

        config.requireLoading = true;
        config.requireConfirmation = true; //put default value
        config.requireNotification = true; //put default value

        this._control.config.buttons.push({ label: "Update", action: config, icon: { 'fal': true, 'fa-pencil': true }, hide: config.disabled, orderyBy: 2 });
        return this;
    }

    DeleteButton(config: IActionAnimationConfig) {

        config.requireLoading = true;
        config.requireConfirmation = true; //put default value
        config.requireNotification = true; //put default value
        config.validate = () => true;
        this._control.config.buttons.push({ label: "Delete", action: config, icon: { 'fal': true, 'fa-trash': true }, hide: config.disabled, orderyBy: 3 });
        return this;
    }

    PostButton(config: IActionAnimationConfig) {

        config.requireLoading = true;
        config.requireConfirmation = true;
        config.requireNotification = true;

        this._control.config.buttons.push({ label: "Post", action: config, icon: { 'fal': true, 'fal fa-mailbox': true }, hide: config.disabled, orderyBy: 4 });
        return this;
    }

    ReverseButton(config: IActionAnimationConfig) {

        config.requireLoading = true;
        config.requireConfirmation = true;
        config.requireNotification = true;

        this._control.config.buttons.push({ label: "Reverse", action: config, icon: { 'fal': true, 'far fa-redo': true }, hide: config.disabled, orderyBy: 5 });
        return this;
    }

    VoidButton(config: IActionAnimationConfig) {

        config.requireLoading = true;
        config.requireConfirmation = true;
        config.requireNotification = true;

        this._control.config.buttons.push({ label: "Void", action: config, icon: { 'fal': true, 'fa-times': true }, hide: config.disabled, orderyBy: 6 });

        return this;
    }

    RecapButton(config: IActionAnimationConfig) {

        config.requireLoading = true;
        config.requireConfirmation = false;
        config.requireNotification = false;

        this._control.config.buttons.push({ label: "Recap", action: config, icon: { 'fal': true, 'fa-recycle': true }, orderyBy: 4.5 });
        return this;
    }
}