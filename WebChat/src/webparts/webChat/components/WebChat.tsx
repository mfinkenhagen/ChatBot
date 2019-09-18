import { DirectLine } from 'botframework-directlinejs';
import * as React from 'react';
import { IWebChatProps } from './IWebChatProps';
import { escape } from '@microsoft/sp-lodash-subset';
import ReactWebChat from 'botframework-webchat';



export default class WebChat extends React.Component<IWebChatProps, {}> {
  private directLine = new DirectLine({token:'XJTej9F_l3Q.f_4jzEt_cXMsBvm4gUhGxVoi9oP3SqwMmsVB7IvMu3s'});
  constructor(props){
    super(props);
    
  }

  public render(): React.ReactElement<IWebChatProps> {
    return (
      <div>
      <iframe 
        src='https://webchat.botframework.com/embed/mf-D3vB0t?s=XJTej9F_l3Q.f_4jzEt_cXMsBvm4gUhGxVoi9oP3SqwMmsVB7IvMu3s'  
        style={{minWidth: '400px', width: '100%', minHeight: '500px'}}></iframe>
      </div>
    );
  }
}
