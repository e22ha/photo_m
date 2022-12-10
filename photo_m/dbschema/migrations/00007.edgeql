CREATE MIGRATION m1f43utzd7q43q6vmtanfc3gypkoguggaqijgwgyc3zuzk2me37zqa
    ONTO m1xruhlkjb52ip5jyv42iwkpcfx6ptifdlbd7oziyy5whbaxmxllaq
{
  DROP FUNCTION default::count_p_by_author(author: default::Photographer);
  CREATE FUNCTION default::count_p_by_author(author_id: std::uuid) ->  std::int64 USING (SELECT
      std::count((SELECT
          default::Photo
      FILTER
          (.author.id = author_id)
      ))
  );
};
