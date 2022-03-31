import CardModel from "./CardModel";

export default class UserModel {
    public id: string = '00000000-0000-0000-0000-000000000000';
    public roomId: string = '00000000-0000-0000-0000-000000000000';
    public name: string = '';
    public isAdmin: boolean = false;
    public points: number = 0;
    public isPlayerTurn: boolean = false;
    public isPlayerCardChar: boolean = false;
    public cards: CardModel[] = [];
    public connectionId: string = '';
  }