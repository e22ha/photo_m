CREATE MIGRATION m1pvdgf6rwqprr6tmtn3uwqn76dzpmsonyje3fuwtv43fkneks5vpa
    ONTO m1rdx3pu5oolp3fseas3w4zsccf42f4jvf7ig5ddv4ttps4komixzq
{
  ALTER TYPE default::Camera {
      CREATE CONSTRAINT std::exclusive ON ((.brand, .model));
  };
};
