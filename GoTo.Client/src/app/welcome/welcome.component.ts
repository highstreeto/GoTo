import { Component, OnInit } from '@angular/core';
import { TripOffer, TripOfferService } from 'src/api-client';

@Component({
  selector: 'goto-welcome',
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.css']
})
export class WelcomeComponent implements OnInit {

  currentTrips: TripOffer[];

  constructor(private tripOfferService: TripOfferService) {
    this.tripOfferService.query()
      .subscribe(trips =>
        this.currentTrips = trips
          .slice(0, 3)
          .sort((a, b) => a.starTime == b.starTime ? 0
            : a.starTime < b.starTime ? -1 : 1)
      )
  }

  ngOnInit() {
  }

}
