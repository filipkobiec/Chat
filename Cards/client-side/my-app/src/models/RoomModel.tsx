import CardModel from "./CardModel";
import UserModel from "./UserModel";

export default class RoomModel {
    public id: string = '';
    public roomName: string = '';
    public admin: UserModel | undefined;
    public cardChar: UserModel | undefined;
    public userModels: UserModel[] = [];
    public blackCard: CardModel | undefined;
    public blackCards: CardModel[] = [];
    public chosenCards: CardModel[] = [];
  }