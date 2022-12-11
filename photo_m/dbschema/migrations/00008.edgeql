CREATE MIGRATION m1ktjjd42cdxlpxxs5kkq3wjkj3j2nybtxms45te3w2axqdvdpqq2q
    ONTO m1f43utzd7q43q6vmtanfc3gypkoguggaqijgwgyc3zuzk2me37zqa
{
  ALTER TYPE default::Photo {
      ALTER PROPERTY full_path {
          USING ((.directory ++ .name));
      };
  };
};
