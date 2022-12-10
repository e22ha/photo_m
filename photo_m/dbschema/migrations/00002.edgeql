CREATE MIGRATION m15c6a3lw4jqk4ekao2itv3gtpq2adxg5tatyxn7oa7wlkeojcbvba
    ONTO m17mw3bfip6dzrl7f4y7u24mkbprjefbl6gvvtz6arwqii7y4injta
{
  ALTER TYPE default::Human {
      CREATE PROPERTY nick -> std::str;
  };
};
