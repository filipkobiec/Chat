import CardModel from "./CardModel";

export default class UserModel {
    public name: string = '';
    public isAdmin: boolean = false;
    public points: number = 0;
    public isPlayerTurn: boolean = false;
    public cards: CardModel[] = [];
  }