import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ResponsePredict } from 'src/app/models/ResponsePredict';
import { ResponseDialogComponent } from '../response-dialog/response-dialog.component';

@Component({
  selector: 'app-predict-card',
  templateUrl: './predict-card.component.html',
  styleUrls: ['./predict-card.component.scss']
})
export class PredictCardComponent implements OnInit {

  baseUri: string = "https://localhost:7115/Neural/PredictFlower?";

  flowerForm = new FormGroup({
    sepalLength: new FormControl('', Validators.required),
    sepalWidth: new FormControl('', Validators.required),
    petalLength: new FormControl('', Validators.required),
    petalWidth: new FormControl('', Validators.required),
  });

  constructor(public dialog: MatDialog) { }

  ngOnInit() {
  }


  async predictFlower() {
    const response = await fetch(
      `${this.baseUri}sepalLength=${this.flowerForm.value.sepalLength}&sepalWidth=${this.flowerForm.value.sepalWidth}&petalLength=${this.flowerForm.value.petalLength}&petalWidth=${this.flowerForm.value.petalWidth}`
    );
    const result: ResponsePredict = await response.json();
    console.log(result.response);

    const dialogRef = this.dialog.open(ResponseDialogComponent, {
      width: '350px',
      backdropClass: 'popup-backdrop',
      data: result.response
    });
    dialogRef.afterClosed().subscribe(result => {
      this.flowerForm.reset();
    });
  }

}
