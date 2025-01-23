

export const UrlBuilder = {
    getBaseUrl: (): string => {
      if (window.location.hostname === 'localhost') {
        return process.env.REACT_APP_LOCAL_SERVER_API_URL as string;
      } else {
        return process.env.REACT_APP_SERVER_API_URL as string;
      }
    },
  
    getMapUrl: (): string => {
      if (window.location.hostname === 'localhost') {
        console.log(process.env.REACT_APP_LOCAL_MAP_API_URL)
        return process.env.REACT_APP_LOCAL_MAP_API_URL as string;
      } else {
        return process.env.REACT_APP_MAP_API_URL as string;
      }
    },
};
