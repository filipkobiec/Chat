import UserModel from "./UserModel";

export default class RoomModel {
    public id: string = '';
    public roomName: string = '';
    public admin: UserModel | undefined;
    public userModels: UserModel[] = [];
  }