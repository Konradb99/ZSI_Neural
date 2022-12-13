export class ResponsePredict {
    response: string;
    constructor();
    constructor(
        response: string
    );
    constructor(
        response?: string
    ) {
      this.response = response ?? "";
    }
  }
  