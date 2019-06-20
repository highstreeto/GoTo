import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TripOfferService, TripOffer } from 'src/api-client';
import { Validators } from '@angular/forms';
import { of } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'goto-trip-add',
  templateUrl: './trip-add.component.html',
  styleUrls: ['./trip-add.component.css']
})
export class TripAddComponent implements OnInit {

  tripForm = new FormGroup({
    startLoc: new FormControl('', [
      Validators.required,
      Validators.minLength(3)
    ]),
    startTime: new FormControl(this.now(), [
      Validators.required,
    ]),
    endLoc: new FormControl('', [
      Validators.required,
      Validators.minLength(3)
    ]),
    endTime: new FormControl(this.now(), [
      Validators.required
    ]),
    user: new FormControl('', [
      Validators.required,
      Validators.minLength(3)
    ])
  });
  serverError: string = "";
  submitting: boolean = false;

  constructor(
    private tripOfferService: TripOfferService,
    private router: Router
  ) {

  }

  ngOnInit() {
  }

  get startLoc() { return this.tripForm.get('startLoc'); }
  get startTime() { return this.tripForm.get('startTime'); }
  get endLoc() { return this.tripForm.get('endLoc'); }
  get endTime() { return this.tripForm.get('endTime'); }
  get user() { return this.tripForm.get('user'); }

  private now(): string {
    let date = new Date();
    date.setMilliseconds(0); // remove ms -> not needed
    date.setSeconds(0); // remove s -> not needed

    return date.toISOString().slice(0, -1);
  }

  onSubmit() {
    let offer: TripOffer = {
      starTime: this.startTime.value,
      startLocation: this.startLoc.value,
      endTime: this.endTime.value,
      endLocation: this.endLoc.value,
      offeredBy: this.user.value,
    };
    console.log(offer);

    this.submitting = true;
    this.tripOfferService.add(offer).subscribe(
      added => {
        this.submitting = false;
        this.router.navigate(['/'])
      },
      error => {
        console.log("could not submit: " + JSON.stringify(error));
        this.submitting = false;
        switch (error.status) {
          case 0:
            this.serverError = "Server kann nicht erreicht werden!";
            break;
          case 400:
            this.serverError = error.error;
            break;
        }
      }
    );
  }
}
