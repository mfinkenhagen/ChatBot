import { DirectLine } from 'botframework-directlinejs';
import * as React from 'react';
import { IWebChatProps } from './IWebChatProps';
import { escape } from '@microsoft/sp-lodash-subset';
import ReactWebChat from 'botframework-webchat';



export default class WebChat extends React.Component<IWebChatProps, {}> {
  private directLine = new DirectLine({token:''});
  constructor(props){
    super(props);
    
  }

  public render(): React.ReactElement<IWebChatProps> {
    return (
      <ReactWebChat directLine={ this.directLine } userID='YOUR_USER_ID' />
      
    );
  }
}
